import React from 'react';
import { Modal, Button } from "react-bootstrap";
import axios from 'axios';

class CreateLambda extends React.Component {

    constructor(props) {
        super(props);

        this.createLambda = this.createLambda.bind(this);
        this.onChange = this.onChange.bind(this);

        this.state = {
            file: null,
            lambdaName: '',
            runtime: '',
            trigger: ''
        }
    }

    createLambda(event) {

        event.preventDefault();

        const options = {
            headers: { 'token': localStorage.getItem("token") }
        };



        var lambdaName = document.getElementById("lambdaName").value;
        var runtime = document.getElementById("runtime").value;
        var trigger = document.getElementById("trigger").value;

        var formData = new FormData();

        formData.append('file', this.state.file);
        formData.append('Name', lambdaName);
        formData.append('Runtime', runtime);
        formData.append('Trigger', trigger);

        axios.post("http://localhost:65001/api/lambda/create", formData , options).then(
            (resp) => this.onSuccessHandler(resp),
            (resp) => this.onErrorHandler(resp)
        );

      
    }


    onErrorHandler(resp) {
        alert(resp.response.data);
    }

    onSuccessHandler(resp) {
        this.props.addLambda(resp.data.name);
        this.props.onHide();
    }

    onChange(e) {
        e.preventDefault();
        this.setState({ file: e.target.files[0] })
     

        
    }


    render() {
        return (
            <div>
                <Button variant="primary" onClick={() => this.props.onHide(true)}>
                    Create Lambda
                </Button>
                <Modal
                    show={this.props.show}
                    size="lg"
                    aria-labelledby="contained-modal-title-vcenter"
                    centered = "true"
                >
                    <Modal.Header>
                        <Modal.Title id="contained-modal-title-vcenter">
                            New Lambda
                        </Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <form onSubmit={this.createLambda} id="lambdaForm">
                            <div className="form-group">
                                <input type="text"
                                    className="form-control form-control-sm"
                                    id="lambdaName"
                                    name="newLambda"
                                    placeholder="Enter name"
                                    required
                                />
                                <br/>
                                <select id="runtime" name="runtime" className="form-control form-control-sm" required>
                                    <option value="CSHARP">.NET Core 2.1</option>
                                    <option value="PYTHON">Python 3</option>
                                </select>
                                <br />
                                <select id="trigger" name="trigger" className="form-control form-control-sm" required>
                                    <option value="GET">GET</option>
                                </select>
                                <br />
                                <div>
                                    <label>Select File</label>
                                    <input type="file" name="file" id="file" onChange={this.onChange} />
                                </div>
                            </div>
                            <hr/>
                            <Button type="submit">Create</Button>
                            <Button onClick={() => this.props.onHide(false)}>Close</Button>
                        </form>
                    </Modal.Body>
                </Modal>
            </div>
        );
    }

}

export default CreateLambda;